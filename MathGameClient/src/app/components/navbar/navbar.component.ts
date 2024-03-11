import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { finalize } from 'rxjs';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { PlayerService } from 'src/app/services/player.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css']
})
export class NavbarComponent implements OnInit {
  constructor(public playerService: PlayerService, private router: Router, private toastr: ToastrService) {

   }
   showLogoutButton: boolean = true
   showBackToHomePage: boolean = false;


  ngOnInit(): void {
    const userHasToken = this.playerService.isAuthenticated(); {
      if(!userHasToken) {
        this.showLogoutButton = false
      } else {
        this.showLogoutButton = true;
      }
    }
  }

  logout() {
    this.playerService.logoutPlayer()
      .pipe(
        finalize(() => {
          this.router.navigateByUrl('/');
          this.playerService.setCurrentPlayer(false);
          this.showLogoutButton = false;
        })
      )
      .subscribe({
        next: response => {
          if (response.success) {
            this.toastr.success(ErrorMessages.logoutSuccess);
          } else {
            this.toastr.error(response.errorMessage || ErrorMessages.genericUnknownError);
          }
        }
      });
  }

  goToGame() {
    this.showBackToHomePage = true;
    this.router.navigateByUrl('game')
  }

  goToHomePage() {
    this.showBackToHomePage = false;
    this.router.navigateByUrl('home')
  }
}

