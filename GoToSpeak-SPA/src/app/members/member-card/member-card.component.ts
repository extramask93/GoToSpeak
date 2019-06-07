import { Component, OnInit, Input, Output } from '@angular/core';
import { User } from 'src/app/_models/user';
import { EventEmitter } from '@angular/core';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  @Input() user: User;
  @Output() userSelectedEvent = new EventEmitter();
  constructor() { }

  ngOnInit() {
  }

}
