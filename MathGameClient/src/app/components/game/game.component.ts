import { Component, OnInit } from '@angular/core';
import { GameExpression } from 'src/app/models/gameExpression';
import { GameExpressionService } from 'src/app/services/gameExpression.service';
import { AnswerStatus } from 'src/app/enums/AnswerStatus';
import { PlayerInfoModel } from 'src/app/models/playerInfoModel';
import { PlayerService } from 'src/app/services/player.service';
import { ServiceResponse } from 'src/app/models/serviceResponse';
import { Observable } from 'rxjs';
import { SignalRService } from 'src/app/services/signalR.service';

@Component({
  selector: 'app-game',
  templateUrl: './game.component.html',
  styleUrls: ['./game.component.css']
})
export class GameComponent implements OnInit {
  gameExpressions: GameExpression[] = [];
  AnswerStatus = AnswerStatus;
  playerInfo: PlayerInfoModel | undefined;
  guesss: boolean = false;
  playerScore: number = 0 ;
  onlinePlayers: number = 0;
  constructor(
    private gameExpressionService: GameExpressionService,
    private playerService: PlayerService,
    private signalRService: SignalRService
  ) { }

  ngOnInit(): void {
    this.getPlayerInfo().subscribe(response => {
      if (response.success && response.data) {
        this.playerInfo = response.data;
        const gameSessionId = this.playerInfo.gameSessionId;
        this.playerScore = this.playerInfo.playerScore;
        this.getGameExpressionsForSession(gameSessionId);
        this.fetchOnlinePlayers();
        this.startListeningForPlayerCountUpdates();
      }
    });

    this.signalRService.startConnection();

    this.signalRService.onDataUpdate(this.handleNewOrUpdatedExpression);
  }

  getPlayerInfo(): Observable<ServiceResponse<PlayerInfoModel>> {
    return this.playerService.getPlayerInfo();
  }

  getGameExpressionsForSession(gameSessionId: number): void {
    this.gameExpressionService.getAllGameExpressionsForGameSession(gameSessionId)
      .subscribe(expressions => {
        this.gameExpressions = expressions;
      });
  }

  guessExpression(event: any, expressionId: number): void {
    const answer = (event.target as HTMLInputElement).value;
    const model = {
      expressionId: expressionId,
      guess: parseFloat(answer),
      email: this.playerInfo?.email || ''
    };
    this.gameExpressionService.answerExpression(model)
      .subscribe(response => {
        if (response.success) {
          this.playerScore = response.data ?? 0; 
        }
      });
  }
  handleNewOrUpdatedExpression = (expression: GameExpression) => {
    if (this.playerInfo?.gameSessionId === expression.gameSessionId) {
      const existingExpressionIndex = this.gameExpressions.findIndex(exp => exp.id === expression.id);
      if (existingExpressionIndex !== -1) {
        this.gameExpressions[existingExpressionIndex] = expression;
      } else {
        this.gameExpressions.push(expression);
      }
    }
  };

  startListeningForPlayerCountUpdates(): void {
    this.signalRService.onPlayerCountUpdate((count: number) => {
      this.onlinePlayers = count;
    });
  }

  fetchOnlinePlayers(): void {
    this.playerService.getOnlinePlayers().subscribe(
      (onlinePlayersCount: number) => {
        this.onlinePlayers = onlinePlayersCount;
      }
    );
  }
}