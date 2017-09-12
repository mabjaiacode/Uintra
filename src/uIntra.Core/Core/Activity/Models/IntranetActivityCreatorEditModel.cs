using System.Collections.Generic;
using System.Linq;
using uIntra.Core.Links;
using uIntra.Core.User;

namespace uIntra.Core.Activity
{
    public class IntranetActivityCreatorEditModel
    {
        public IIntranetUser Creator { get; set; }

        public IEnumerable<IIntranetUser> Users { get; set; } = Enumerable.Empty<IIntranetUser>();

        public bool CanEditCreator { get; set; }

        public string CreatorIdPropertyName { get; set; }
        public ActivityCreateLinks Links { get; set; }
    }
}