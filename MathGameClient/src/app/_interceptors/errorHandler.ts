import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpResponse,
  HttpErrorResponse
} from '@angular/common/http';
import { Observable, throwError, of } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ErrorMessages } from '../helpers/errorMessages';
import { environment } from 'src/environments/environment';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private toastr: ToastrService, private router: Router) {}

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request).pipe(
      catchError((error: HttpErrorResponse) => {
        let errorMessage = ErrorMessages.genericUnknownError;

        if (error.status === 0) {

          errorMessage = ErrorMessages.serverError;
        } else {
          if (error.error instanceof ErrorEvent) {

            errorMessage = `Error: ${error.error.message}`;
          } else {
            switch (error.status) {
              case 400:
                errorMessage = ErrorMessages.unauthorizedAccess;
                this.router.navigateByUrl('/');
                break;
              case 401:
                errorMessage = ErrorMessages.unauthorizedAccess;
                this.router.navigateByUrl('/');
                break;
              case 404:
                errorMessage = ErrorMessages.resourceNotFound;
                this.router.navigateByUrl('/');
                break;
              case 429:
                errorMessage = ErrorMessages.tooManyRequest;
                this.router.navigateByUrl('/');
                break;
              case 500:
                errorMessage = ErrorMessages.Internalservererror;
                this.router.navigateByUrl('/');
                break;
              default:
                errorMessage = `Error ${error.status}: ${error.message}`;
                break;
            }
          }
        }
        this.toastr.error(errorMessage, 'Error');
        return throwError(errorMessage);
      }),
      retry(environment.failedHttpRetryTimes) 
    );
  }
}
