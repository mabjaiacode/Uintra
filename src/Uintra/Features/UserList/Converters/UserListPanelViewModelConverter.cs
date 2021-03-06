﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Compent.Extensions;
using UBaseline.Core.Extensions;
using UBaseline.Core.Node;
using UBaseline.Core.RequestContext;
using Uintra.Core.Member.Abstractions;
using Uintra.Core.Member.Entities;
using Uintra.Core.Member.Models;
using Uintra.Core.Member.Services;
using Uintra.Core.Search.Entities;
using Uintra.Core.Search.Helpers;
using Uintra.Core.Search.Indexes;
using Uintra.Features.Groups.Services;
using Uintra.Features.Links;
using Uintra.Features.Search.Queries;
using Uintra.Features.UserList.Helpers;
using Uintra.Features.UserList.Models;
using Uintra.Infrastructure.Extensions;

namespace Uintra.Features.UserList.Converters
{
    public class UserListPanelViewModelConverter : INodeViewModelConverter<UserListPanelModel, UserListPanelViewModel>
    {
        private const int AmountPerRequest = 10;

        private readonly IElasticMemberIndex<SearchableMember> _elasticIndex;
        private readonly IProfileLinkProvider _profileLinkProvider;
        private readonly IGroupService _groupService;
        private readonly IIntranetMemberService<IntranetMember> _intranetMemberService;
        private readonly IGroupMemberService _groupMemberService;
        private readonly IUBaselineRequestContext _baselineRequestContext;

        public UserListPanelViewModelConverter(
            IIntranetMemberService<IntranetMember> intranetMemberService,
            IElasticMemberIndex<SearchableMember> elasticIndex,
            IProfileLinkProvider profileLinkProvider,
            IGroupService groupService,
            IGroupMemberService groupMemberService,
            IUBaselineRequestContext baselineRequestContext)
        {
            _elasticIndex = elasticIndex;
            _profileLinkProvider = profileLinkProvider;
            _groupService = groupService;
            _intranetMemberService = intranetMemberService;
            _groupMemberService = groupMemberService;
            _baselineRequestContext = baselineRequestContext;
        }

        public void Map(UserListPanelModel node, UserListPanelViewModel viewModel)
        {
            var groupId = HttpUtility.ParseQueryString(_baselineRequestContext.NodeRequestParams.NodeUrl.Query).TryGetQueryValue<Guid?>("groupId");
            viewModel.Details = GetUsers(groupId);
        }

        private  MembersRowsViewModel GetUsers(Guid? groupId)
        {
            var listSearch = new ActiveMemberSearchQuery
            {
                GroupId = groupId,
                OrderingString = string.Empty,
                Text = string.Empty,
                Page = 1,
                MembersOfGroup = groupId.HasValue
            };

            var (activeUsers, isLastRequest) = GetActiveUsers(listSearch, groupId);

            var model = GetUsersRowsViewModel(groupId);

            model.SelectedColumns =
                UsersPresentationHelper.ExtendIfGroupMembersPage(listSearch.GroupId,
                    UsersPresentationHelper.GetProfileColumns());
            model.Members = activeUsers;
            model.IsLastRequest = isLastRequest;

            return model;
        }

        private (IEnumerable<MemberModel> result, bool isLastRequest) GetActiveUsers(ActiveMemberSearchQuery query,
            Guid? groupId)
        {
            var (searchResult, totalHits) = GetActiveUserIds(query);

            var result = _intranetMemberService.GetMany(searchResult)
                .Select(x => MapToViewModel(x, groupId));

            var skip = (query.Page - 1) * AmountPerRequest;

            var isLastRequest = skip + AmountPerRequest >= totalHits;

            return (result, isLastRequest);
        }

        private (IEnumerable<Guid> searchResult, long totalHits) GetActiveUserIds(
            ActiveMemberSearchQuery query)
        {
            var skip = (query.Page - 1) * AmountPerRequest;

            var searchQuery = new MemberSearchQuery
            {
                Text = query.Text,
                Skip = skip,
                Take = AmountPerRequest,
                OrderingString = ElasticHelpers.FullName,
                SearchableTypeIds = ((int) UintraSearchableTypeEnum.Member).ToEnumerable(),
                GroupId = query.GroupId,
                MembersOfGroup = query.MembersOfGroup
            };

            var searchResult = _elasticIndex.Search(searchQuery);
            var result = searchResult.Documents.Select(r => Guid.Parse(r.Id.ToString()));

            return (result, searchResult.TotalHits);
        }

        private MembersRowsViewModel GetUsersRowsViewModel(Guid? groupId)
        {
            var model = new MembersRowsViewModel
            {
                SelectedColumns = UsersPresentationHelper.GetProfileColumns(),
                CurrentMember = _intranetMemberService.GetCurrentMember().ToViewModel(),
            };

            model.IsCurrentMemberGroupAdmin = groupId.HasValue && _groupMemberService
                                                  .IsMemberAdminOfGroup(model.CurrentMember.Id, groupId.Value);

            model.GroupId = groupId;

            return model;
        }

        private MemberModel MapToViewModel(IIntranetMember user, Guid? groupId)
        {
            var model = user.Map<MemberModel>();
            model.ProfileUrl = _profileLinkProvider.GetProfileLink(user.Id);

            var isAdmin = groupId.HasValue && _groupMemberService
                              .IsMemberAdminOfGroup(user.Id, groupId.Value);

            model.IsGroupAdmin = isAdmin;
            model.IsCreator = groupId.HasValue && _groupService.IsMemberCreator(user.Id, groupId.Value);

            return model;
        }
    }
}