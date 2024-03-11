// http.interceptor.ts
import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpHandler, HttpRequest } from '@angular/common/http';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const clonedReq = req.clone({
      withCredentials: true
    });

    return next.handle(clonedReq);
  }
}