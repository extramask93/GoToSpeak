import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import {HttpClientModule} from '@angular/common/http';
import { NavComponent } from './nav/nav.component';
import {FormsModule, ReactiveFormsModule} from '@angular/forms';
import { AuthService } from './_services/auth.service';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { ErrorInterceptorProvider } from './_services/error.interceptor';
import { AlertifyService } from './_services/alertify.service';
import { BsDropdownModule } from 'ngx-bootstrap';
import { RouterModule } from '@angular/router';
import { appRoutes } from './routes';
import { AuthGuard } from './_guards/auth.guard';
import { ChatService } from './_services/chat.service';
import { MemberListComponent } from './members/memberList/memberList.component';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { JwtModule } from '@auth0/angular-jwt';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver.';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
import { SignalRService } from './_services/signalR.service';
import { UsersListComponent } from './users-list/users-list.component';
import { PhotoEditorComponent } from './photo-editor/photo-editor.component';
import { FileUploadModule } from 'ng2-file-upload';
import { UserResolver } from './_resolvers/user.resolver';
import { TimeAgoPipe } from 'time-ago-pipe';
import { WelcomeComponent } from './welcome/welcome.component';
import { LobbyComponent } from './lobby/lobby/lobby.component';
import { RoomListComponent } from './lobby/room-list/room-list.component';
import {NgbModule} from '@ng-bootstrap/ng-bootstrap';
import { RoomCardComponent } from './lobby/room-list/room-card/room-card.component';
import { UserCardComponent } from './lobby/user-list/user-card/user-card.component';
import { UserListComponent } from './lobby/user-list/user-list.component';
import { RoomServiceService } from './_services/room-service.service';
import { ChatWindowComponent } from './lobby/chat-window/chat-window.component';
import { RoomResolver } from './_resolvers/room.resolver';
import { ListResolver } from './_resolvers/list.resolver';
import { ChatResolver } from './_resolvers/chat.resolver';

export function tokenGetter() {
   return localStorage.getItem('token');
}

@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      HomeComponent,
      RegisterComponent,
      MemberListComponent,
      MemberCardComponent,
      MemberMessagesComponent,
      UsersListComponent,
      PhotoEditorComponent,
      TimeAgoPipe,
      WelcomeComponent,
      LobbyComponent,
      RoomListComponent,
      RoomCardComponent,
      UserCardComponent,
      UserListComponent,
      ChatWindowComponent
   ],
   imports: [
      BrowserModule,
      AppRoutingModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      JwtModule.forRoot({
         config: {
            // tslint:disable-next-line:object-literal-shorthand
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000'],
            blacklistedRoutes: ['localhost:5000/api/auth']
         }
      }),
      BsDropdownModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      FileUploadModule,
      NgbModule
   ],
   providers: [
      AuthService,
      AlertifyService,
      ChatService,
      ErrorInterceptorProvider,
      AuthGuard,
      MemberListResolver,
      SignalRService,
      RoomServiceService,
      MessagesResolver,
      UserResolver,
      RoomResolver,
      ListResolver,
      ChatResolver
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
