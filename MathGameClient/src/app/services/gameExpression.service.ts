import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from 'src/environments/environment';
import { GameExpression } from '../models/gameExpression';
import { AnswerExpressionRequestModel } from '../models/answerExpressionRequestModel ';
import { ServiceResponse } from '../models/serviceResponse';


@Injectable({
  providedIn: 'root'
})
export class GameExpressionService {
  private baseUrl = environment.apiUrl + 'GameExpression'; 
  constructor(private http: HttpClient) { }

  getAllGameExpressionsForGameSession(gameSessionId: number): Observable<GameExpression[]> {
    return this.http.get<GameExpression[]>(`${this.baseUrl}/game-session/${gameSessionId}`);
  }

  answerExpression(model: AnswerExpressionRequestModel): Observable<ServiceResponse<number>> {
    return this.http.post<ServiceResponse<number>>(`${this.baseUrl}/answer-expression`, model)
  }
}