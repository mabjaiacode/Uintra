import { Component, Input, OnInit } from '@angular/core';
import { IULink } from 'src/app/shared/interfaces/general.interface';

@Component({
  selector: 'app-publication-header',
  templateUrl: './publication-header.component.html',
  styleUrls: ['./publication-header.component.less']
})
export class PublicationHeaderComponent implements OnInit {

  @Input() avatar: string;
  @Input() title: string;
  @Input() link: any;
  @Input() params?: Array<any>;
  @Input() groupInfo?: {title: string, url: IULink};

  ngOnInit(): void {

  }

}
