import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  messageReceived = new EventEmitter<Message>();
  globalMessageReceived = new EventEmitter<Message>();
  historyReceived = new EventEmitter<Message[]>();
  usersReceived = new EventEmitter<User[]>();
  userJoined = new EventEmitter<User[]>();
  userLeft = new EventEmitter<User[]>();
  // tslint:disable-next-line:ban-types
  connectionEstablished = new EventEmitter<boolean>();
  public data: any;
  private newMessage: any = {};
  private connectionIsEstablished = false;
  private hubConnection: signalR.HubConnection;
  baseUrl = environment.sigRUrl;

  constructor() {
    this.createConnection();
    this.registerOnServerEvents();
    this.startConnection();
  }
  private createConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this.baseUrl + 'temp', {
      accessTokenFactory: () => {
          return localStorage.getItem('token');
      }
    })
    .build();
  }
  private registerOnServerEvents(): void {
    this.hubConnection.on('NewMessage', (data: any) => {
      this.messageReceived.emit(data);
    });
    this.hubConnection.on('NewGlobalMessage', (data: any) => {
      this.globalMessageReceived.emit(data);
    })
    this.hubConnection.on('ActiveUsers', (data: any) => {
      this.usersReceived.emit(data);
    });
    this.hubConnection.on('MessageHistory', (data: any) => {
      this.historyReceived.emit(data);
    });
    this.hubConnection.on('UserLeft', (data: any) => {
      this.userLeft.emit(data);
    });
    this.hubConnection.on('UserJoined', (data: any) => {
      this.userJoined.emit(data);
    });
    this.hubConnection.onclose((error) => {this.hubConnection.start(); });
  }
  private startConnection(): void {
    this.hubConnection
      .start()
      .then(() => {
        this.connectionIsEstablished = true;
        console.log('Hub connection started');
        this.connectionEstablished.emit(true);
      })
      .catch(err => {
        console.log('Error while establishing connection, retrying...');
      });
  }
  public sendGlobalMessage(message: any) {
    this.hubConnection.invoke('SendGlobalMessage', message);
  }
  public sendMessage(message: any): void {
    this.hubConnection.invoke('SendMessage', message);
  }
}
/*

*/
