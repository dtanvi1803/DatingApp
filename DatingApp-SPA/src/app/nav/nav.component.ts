import { Component, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {
  model: any = {};
  constructor(public authService: AuthService, private alertifyService: AlertifyService) { }

  ngOnInit() {
  }
  login() {
    this.authService.login(this.model).subscribe(next => {
      this.alertifyService.success('Logged in Sucessfully!!');
    }, error => {
      this.alertifyService.error(error);
    });
   }

  loggendIn() {
    return this.authService.loggedIn();
  }
  logout() {
    localStorage.removeItem('token');
    this.alertifyService.message('logged out');
  }
}
