import {Component, OnInit} from '@angular/core';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {AuthService} from '../../auth/services/auth.service';

@Component({
  selector: 'sidebar',
  templateUrl: 'sidebar.component.html',
  styleUrl: 'sidebar.component.css',
  imports: [
    RouterLink,
    RouterLinkActive
  ]
})
export class SidebarComponent implements OnInit {
  constructor(private userService: AuthService) { }

  userName: string = "";

  ngOnInit() {
    this.userName = this.getUserName()
  }

  getUserName(): string {
    return this.userService.getUserInfo()?.name ?? "Account";
  }

}
