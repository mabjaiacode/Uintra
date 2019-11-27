import { Component, OnInit } from '@angular/core';

interface ITagData {
  id: string;
  text: string;
}

@Component({
  selector: 'app-tag-multiselect',
  templateUrl: './tag-multiselect.component.html',
  styleUrls: ['./tag-multiselect.component.less']
})
export class TagMultiselectComponent implements OnInit {
  isDwopdownShowed: boolean = false;
  selectedList: Array<ITagData> = [];
  dropdownList: Array<ITagData> = [];

  constructor() { }

  onShowDropdown() {
    this.isDwopdownShowed = true;
  }

  onHideDropdown() {
    this.isDwopdownShowed = false;
  }

  onAddTag(tag) {
    if(this.selectedList.includes(tag)) return;
    this.selectedList.push(tag);
    this.onHideDropdown();
  }

  onRemoveTag(tag) {
    this.selectedList = this.selectedList.filter(curTag => curTag.id !== tag.id);
  }

  onClearSelectedTags() {
    this.selectedList = [];
    this.onHideDropdown();
  }

  ngOnInit() {
    this.dropdownList = [
      { id: '1', text: 'Mumbai' },
      { id: '2', text: 'Lolkek' },
      { id: '3', text: 'Mumbai2' },
      { id: '4', text: 'Lolkek2' },
      { id: '5', text: 'Mumbai3' },
      { id: '6', text: 'Lolkek3' }
    ];
  }
}
