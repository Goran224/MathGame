import { Component, OnInit } from '@angular/core';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { LoginModel } from 'src/app/models/loginModel';
import { PlayerService } from 'src/app/services/player.service';
import { ToastrService } from 'ngx-toastr';
import { ServiceResponse } from 'src/app/models/serviceResponse';
import { LoginResponseModel } from 'src/app/models/loginResponseModel';
import { Router } from '@angular/router';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
})
export class LoginComponent implements OnInit {
  constructor(
    public playerService: PlayerService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  showSecurityCodeField: boolean = false;
  showSpinner: boolean = false;

  loginModel: LoginModel = {
    email: '',
    password: '',
  };

  ngOnInit(): void {    }

  login() {
    this.showSpinner = true;
    this.playerService
      .loginPlayer(this.loginModel)
      .subscribe((response: ServiceResponse<LoginResponseModel>) => {
        if (response.success && response.data) {
          const player: LoginResponseModel = response.data;
          if (!player.isAccountLocked) {
            this.handleSuccessfulLogin(player);
          }
        }
      })
      .add(() => {
        this.showSpinner = false;
      });
  }
  
  handleSuccessfulLogin(playerFromResponse: LoginResponseModel) {
    if (playerFromResponse) {
      const player: LoginResponseModel = playerFromResponse;
      this.playerService.setCurrentPlayer(player !== null);
      this.router.navigateByUrl('home');
      this.toastr.success(ErrorMessages.logInSuccess, 'Success');
    }
  }
}
