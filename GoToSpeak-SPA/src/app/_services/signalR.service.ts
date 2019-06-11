import { Injectable } from '@angular/core';
import * as signalR from '@aspnet/signalr';
@Injectable({
  providedIn: 'root'
})
export class SignalRService {
public data: any;
private newMessage: any = {};
private hubConnection: signalR.HubConnection;
 public startConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
                            .withUrl('http://localhost:5000/temp', {
                              accessTokenFactory: () => {
                                  return localStorage.getItem('token');
                              }
                            })
                            .build();
    this.hubConnection
      .start()
      .then(() => {this.addUsersListener(); this.callRPC();})
      .then(() => console.log('Connection started'))
      .catch(err => console.log('Error while starting connection: ' + err));
  }
  public addUsersListener = () => {
    this.hubConnection.on('ActiveUsers', (data) => {
      this.data = data;
      console.log(data);
    });
    this.hubConnection.on('NewMessage', (data) => {
      console.log(data);
    });
  }
  public callRPC = () => {
    this.newMessage.recipientId = '6';
    this.newMessage.content = 'message from west to west via hub';
    this.hubConnection.invoke('GetActiveUsers');
    this.hubConnection.invoke('SendMessage', this.newMessage);
  }
}
/*

*/
