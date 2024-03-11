import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
import { Player } from '../models/player';
import { LoginModel } from '../models/loginModel';
import { LoginResponseModel } from '../models/loginResponseModel';
import { ServiceResponse } from '../models/serviceResponse';
import { environment } from '../../environments/environment';
import { PlayerInfoModel } from '../models/playerInfoModel';
@Injectable({
  providedIn: 'root'
})
export class PlayerService {
  private baseUrl = environment.apiUrl + 'player'; // Use environment variable
  private hasLoggedUserSubject: BehaviorSubject<boolean>;
  private requestIsFromConfirmingAccountSubject: BehaviorSubject<boolean>;

  constructor(private http: HttpClient) {
    this.hasLoggedUserSubject = new BehaviorSubject<boolean>(false);
    this.requestIsFromConfirmingAccountSubject = new BehaviorSubject<boolean>(false);
   }

   get hasLoggedPlayer$() {
    return this.hasLoggedUserSubject.asObservable();
  }

  get isRequestIsFromConfirmingAccountSubject$() {
    return this.requestIsFromConfirmingAccountSubject.asObservable();
  }

  setCurrentPlayer(playerIsLogged: boolean) {
    if(this.isAuthenticated() && playerIsLogged === null) {
      this.hasLoggedUserSubject.next(true)
    }
    this.hasLoggedUserSubject.next(playerIsLogged);
  }

  registerPlayer(player: Player): Observable<ServiceResponse<number>> {
    return this.http.post<ServiceResponse<number>>(`${this.baseUrl}/register-account`, player);
  }

  loginPlayer(loginModel: LoginModel): Observable<ServiceResponse<LoginResponseModel>> {
    return this.http.post<ServiceResponse<LoginResponseModel>>(`${this.baseUrl}/login-player`, loginModel);
  }

  logoutPlayer(): Observable<ServiceResponse<boolean>> {
    return this.http.post<ServiceResponse<boolean>>(`${this.baseUrl}/logout`, null);
  }

  getPlayerInfo(): Observable<ServiceResponse<PlayerInfoModel>> {
    return this.http.post<ServiceResponse<PlayerInfoModel>>(`${this.baseUrl}/get-logged-player-info`, {});
  }

  isAuthenticated(): Observable<boolean> {
    return this.http.post<boolean>(`${this.baseUrl}/isAuthenticated`, null);
  }

  getOnlinePlayers(): Observable<number> {
    return this.http.post<number>(`${this.baseUrl}/getonlineplayers`, {});
  }
}