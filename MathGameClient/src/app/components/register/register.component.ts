import { Component, EventEmitter, OnInit, Output, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { PlayerService } from 'src/app/services/player.service';
import { ToastrService } from 'ngx-toastr';
import { NgForm } from '@angular/forms';
import { catchError, of } from 'rxjs';
import { ErrorMessages } from 'src/app/helpers/errorMessages';
import { RegisterModel } from 'src/app/models/registerModel';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  constructor(
    private playerService: PlayerService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  @Output() cancelRegister = new EventEmitter();
  @ViewChild('registerForm') registerForm: NgForm | undefined;

  registerModel: RegisterModel = {
    firstName: '',
    lastName: '',
    password: '',
    email: '',
    confirmPassword: '',
  };

  passwordsDontMatch: string = '';
  requestIsLoading: boolean = false;

  ngOnInit(): void {
    this.resetForm();
  }

  register() {
    this.requestIsLoading = true;
    this.passwordsDontMatch = this.passwordCheck();

    if (this.passwordsDontMatch === '') {
      this.playerService
        .registerPlayer(this.registerModel)
        .pipe(
          catchError(() => {
            this.requestIsLoading = false;
            return of(null);
          })
        )
        .subscribe((response) => {
          if (response && response.success) {
            this.toastr.success('Registration successful');
            this.cancel();
          } else {
            const errorMessage = response && response.errorMessage ? response.errorMessage : ErrorMessages.genericErrorWhileVerifyingAccount;
            this.toastr.error(errorMessage, 'Error');
            this.clearForm();
          }
          this.requestIsLoading = false;
        });
    }
  }

  cancel() {
    this.router.navigateByUrl('');
    this.cancelRegister.emit(false);
  }

  passwordCheck(): string {
    if (this.registerModel.password !== this.registerModel.confirmPassword) {
      this.requestIsLoading = false;
      this.toastr.error(ErrorMessages.passwordsDontMatch, 'Error');
      return ErrorMessages.passwordsDontMatch;
    }
    return '';
  }

  clearForm() {
    this.resetForm();
    !this.registerForm?.form.markAsPristine();
  }

  resetForm() {
    this.registerModel.firstName = '';
    this.registerModel.lastName = '';
    this.registerModel.email = '';
    this.registerModel.password = '';
    this.registerModel.confirmPassword = '';
    this.passwordsDontMatch = '';
  }
}