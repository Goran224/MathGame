import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ServiceResponse } from '../models/serviceResponse';
import { GameSessionResponseModel } from '../models/gameSessionResponseModel';
import { environment } from 'src/environments/environment';
import { GameSessionAssignModel } from '../models/gameSessionAsiggnModel';
import { GameExpression } from '../models/gameExpression';


@Injectable({
  providedIn: 'root'
})
export class GameService {
  private baseUrl = environment.apiUrl + 'gamesession'; 
  constructor(private http: HttpClient) { }

  assignPlayerToSession(email: string): Observable<GameSessionResponseModel> {
    const requestBody: GameSessionAssignModel = { playerEmail: email };
    return this.http.post<GameSessionResponseModel>(`${this.baseUrl}/assign-player`, requestBody);
  }
}