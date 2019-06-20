import { Injectable } from '@angular/core';
import { Resolve, Router, ActivatedRouteSnapshot } from '@angular/router';
import { ChatService } from '../_services/chat.service';
import { AlertifyService } from '../_services/alertify.service';
import { Observable, of, from } from 'rxjs';
import { catchError, delay } from 'rxjs/operators';
import { Message } from '../_models/message';
import { AuthService } from '../_services/auth.service';
import { Room } from '../_models/room';
import { SignalRService } from '../_services/signalR.service';
import { RoomServiceService } from '../_services/room-service.service';



@Injectable()
export class ChatResolver implements Resolve<Message[]> {
    rooms: Message[];
    constructor(private signalrService: SignalRService,
                private roomService: RoomServiceService,
                private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Message[]> {
        console.log('running resolver');
        if (this.signalrService.connectionIsEstablished) {
         return from(this.signalrService.getMessageHistory(this.roomService.currentRoom.name));
        }
        return of(null);
    }
}
