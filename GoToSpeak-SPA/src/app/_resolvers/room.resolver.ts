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
export class RoomResolver implements Resolve<Room[]> {
    rooms: Room[];
    pageNumber = 1;
    pageSize = 5;
    constructor(private signalrService: SignalRService,
                private chatService: ChatService,
                private roomService: RoomServiceService,
                private router: Router, private alertify: AlertifyService) {}

    resolve(route: ActivatedRouteSnapshot): Observable<Room[]> {
        return this.chatService.getRooms(this.pageNumber, this.pageSize).pipe(catchError(() => {
            this.alertify.error('Problem retrieving data');
            this.router.navigate(['/home']);
            return of(null);
        }));
    }
}
