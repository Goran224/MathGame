import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { GameExpression } from '../models/gameExpression';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection!: signalR.HubConnection;

  constructor() {}

  startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7166/gamehub')
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started'))
      .catch((err) => console.error('SignalR connection error: ', err));
  };

  onDataUpdate = (callback: (expression: GameExpression) => void): void => {
    this.hubConnection.on(
      'ReceiveNewExpression',
      (expression: GameExpression) => {
        callback(expression);
      }
    );

    this.hubConnection.on(
      'ReceiveUpdatedExpression',
      (expression: GameExpression) => {
        callback(expression);
      }
    );
  };

  onPlayerCountUpdate = (callback: (count: number) => void): void => {
    this.hubConnection.on('ReceiveOnlinePlayersCount', (count: number) => {
      callback(count);
    });
  };
}
