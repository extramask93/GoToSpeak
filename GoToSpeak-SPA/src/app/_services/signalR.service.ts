import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from '@aspnet/signalr';
import { Message } from '../_models/message';
import { Room } from '../_models/room';
import { User } from '../_models/user';
import { environment } from 'src/environments/environment';
import { AlertifyService } from './alertify.service';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
  roomAdded = new EventEmitter<Room>();
  roomDeleted = new EventEmitter<Room>();
  onRoomDeleted = new EventEmitter<string>();
  messageReceived = new EventEmitter<Message>();
  globalMessageReceived = new EventEmitter<Message>();
  historyReceived = new EventEmitter<Message[]>();
  usersReceived = new EventEmitter<User[]>();
  userJoined = new EventEmitter<User>();
  userLeft = new EventEmitter<User>();
  // tslint:disable-next-line:ban-types
  connectionEstablished = new EventEmitter<boolean>();
  public data: any;
  private newMessage: any = {};
  public connectionIsEstablished = false;
  private hubConnection: signalR.HubConnection;
  baseUrl = environment.sigRUrl;

  constructor(private alertify: AlertifyService) {
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
    .configureLogging(signalR.LogLevel.Debug)
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
    this.hubConnection.on('RemoveUser', (data: any) => {
      this.userLeft.emit(data);
    });
    this.hubConnection.on('AddUser', (data: any) => {
      this.userJoined.emit(data);
    });
    this.hubConnection.on('AddChatRoom', (data: any) => {
      this.roomAdded.emit(data);
    });
    this.hubConnection.on('RemoveChatRoom', (data: any) => {
      this.roomDeleted.emit(data);
    });
    this.hubConnection.on('OnRoomDeleted', (data: any) => {
      this.onRoomDeleted.emit(data);
    });
    this.hubConnection.on('OnError',(data: any) => {
      this.alertify.error(data);
    });
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
        setTimeout(this.startConnection, 5000);
      });
  }
  public sendGlobalMessage(message: any) {
    this.hubConnection.invoke('SendGlobalMessage', message);
  }
  public sendMessage(message: any): void {
    this.hubConnection.invoke('SendMessage', message);
  }
  public getRooms() {
    return this.hubConnection.invoke('GetRooms').then((rooms: Room[]) => rooms);
  }
  public joinRoom(roomName: string) {
    return this.hubConnection.invoke('Join', roomName);
  }
  public createRoom(roomName: string) {
    return this.hubConnection.invoke('CreateRoom', roomName);
  }
  public removeRoom(roomName: string) {
    return this.hubConnection.invoke('DeleteRoom', roomName);
  }
  public getUsers(roomName: string) {
    return this.hubConnection.invoke('GetUsers', roomName).then((users: User[]) => users);
  }
  public getMessageHistory(roomName: string) {
    return this.hubConnection.invoke('GetMessageHistory', roomName).then((messages: Message[]) => messages);
  }
  public send(roomName: string, message: string) {
    return this.hubConnection.invoke('Send', roomName, message);
  }
}
/*

*/
