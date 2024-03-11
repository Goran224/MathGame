import { Injectable } from '@angular/core';
import { CanActivate, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { catchError, switchMap, tap } from 'rxjs/operators';
import { GameService } from 'src/app/services/game.service';
import { PlayerService } from 'src/app/services/player.service';
import { ServiceResponse } from '../models/serviceResponse';
import { PlayerInfoModel } from '../models/playerInfoModel';
import { GameSessionResponseModel } from '../models/gameSessionResponseModel';

@Injectable({
  providedIn: 'root'
})
export class GameSessionGuard implements CanActivate {

  constructor(
    private playerService: PlayerService,
    private gameService: GameService,
    private router: Router
  ) {}

  canActivate(): Observable<boolean> {
    return this.playerService.getPlayerInfo().pipe(
      switchMap((response: ServiceResponse<PlayerInfoModel>) => {
        if (response.success && 
          response.data && 
          response.data.gameSessionId && 
          response.data.gameSessionId !== 0) {
          return of(true);
        } else {
          const playerEmail = response.data?.email;
          if (playerEmail) {
            return this.assignPlayerToSessionAndContinue(playerEmail);
          } else {
            return of(false); 
          }
        }
      }),
      catchError(error => {
        this.router.navigateByUrl('home');
        return of(false); 
      })
    );
  }

  private assignPlayerToSessionAndContinue(playerEmail: string): Observable<boolean> {
    return this.gameService.assignPlayerToSession(playerEmail).pipe(
      tap(() => {}),
      switchMap((response: GameSessionResponseModel) => {
        if (response.gameSessionId) {
          return of(true);
        } else {
          this.router.navigateByUrl('home');
          return of(false); 
        }
      }),
      catchError(error => {
        this.router.navigateByUrl('home');
        return of(false); 
      })
    );
  }
}