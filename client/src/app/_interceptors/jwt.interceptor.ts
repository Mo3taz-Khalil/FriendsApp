import { Injectable } from '@angular/core';
import {
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { User } from '../_models/user';
import { take } from 'rxjs/operators';

@Injectable()
export class JWTInterceptor implements HttpInterceptor {

  constructor(private accountservice: AccountService) { }

  intercept(request: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {

    let currentUser: User | null = { username: '', token: '',photoUrl:'' };

    //we want to complete after recieve one current user --take-- so after it complete we are not subscribe to it anymore 
    this.accountservice.currentUser$.pipe(take(1)).subscribe(user => {
      if (user) {
        currentUser = user;
      } else currentUser = null;
    });
    if (currentUser) {
      request = request.clone({
        setHeaders: {
          Authorization: `Bearer ${currentUser.token}`
        }
      })
    }

    return next.handle(request);
  }
}
