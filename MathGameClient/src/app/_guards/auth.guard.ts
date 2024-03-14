import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { PlayerService } from '../services/player.service';
import { map } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private playerService: PlayerService, private router: Router) {}

  canActivate(): Observable<boolean> {
    return this.playerService.isAuthenticated().pipe(
      map((isAuthenticated: boolean) => {
        if (isAuthenticated) {
          return true; 
        } else {
          this.router.navigateByUrl(''); 
          return false; 
        }
      })
    );
  }
}