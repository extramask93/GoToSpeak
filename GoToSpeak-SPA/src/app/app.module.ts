import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
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
import { BsDropdownModule, ModalModule, TabsModule, BsDatepickerModule, PaginationModule } from 'ngx-bootstrap';
import { NgxInfiniteScrollerModule } from 'ngx-infinite-scroller';
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
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/hasRole.directive';
import { UserManagementComponent } from './admin/user-management/user-management.component';
import { AdminService } from './_services/admin.service';
import { SignInComponent } from './sign-in/sign-in.component';
import { SendResetEmailComponent } from './sign-in/send-reset-email/send-reset-email.component';
import { ResetPasswordComponent } from './sign-in/reset-password/reset-password.component';
import { SignInMfaComponent } from './sign-in/sign-in-mfa/sign-in-mfa.component';

import { AccountManagementService } from './_services/account-management.service';
import { LogViewerComponent } from './admin/log-viewer/log-viewer.component';
import { RolesModalComponent } from './admin/roles-modal/roles-modal.component';
import { MfaSetupComponent } from './settings/mfa-setup/mfa-setup.component';
import { SettingsComponent } from './settings/settings.component';
import { PhotoEditorComponent } from './settings/photo-editor/photo-editor.component';
import { MfaRecoveryCodesComponent } from './settings/mfa-recovery-codes/mfa-recovery-codes.component';
import { PasswordChangeComponent } from './settings/password-change/password-change.component';
import { LoginHistoryComponent } from './settings/login-history/login-history.component';
import { MfaDisableComponent } from './settings/mfa-disable/mfa-disable.component';
import { MfaResetComponent } from './settings/mfa-reset/mfa-reset.component';
import { LogsResolver } from './_resolvers/logs.resolver';
import { UsersWithRolesResolver } from './_resolvers/users-with-roles.resolver';
import { LiveButtonComponent } from './nav/live-button/live-button.component';
import { UtcPipePipe } from './_pipes/utcPipe.pipe';
import { UtcStringPipePipe } from './_pipes/utcStringPipe.pipe';

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
      ChatWindowComponent,
      AdminPanelComponent,
      UserManagementComponent,
      HasRoleDirective,
      SignInComponent,
      SendResetEmailComponent,
      ResetPasswordComponent,
      SignInMfaComponent,
      SettingsComponent,
      LoginHistoryComponent,
      MfaSetupComponent,
      MfaRecoveryCodesComponent,
      MfaDisableComponent,
      MfaResetComponent,
      PasswordChangeComponent,
      LogViewerComponent,
      RolesModalComponent,
      LiveButtonComponent,
      UtcPipePipe,
      UtcStringPipePipe
   ],
   imports: [
      BrowserModule,
      BrowserAnimationsModule,
      ModalModule.forRoot(),
      TabsModule.forRoot(),
      BsDatepickerModule.forRoot(),
      PaginationModule.forRoot(),
      AppRoutingModule,
      HttpClientModule,
      FormsModule,
      ReactiveFormsModule,
      JwtModule.forRoot({
         config: {
            // tslint:disable-next-line:object-literal-shorthand
            tokenGetter: tokenGetter,
            whitelistedDomains: ['localhost:5000', 'localhost:5001'],
            blacklistedRoutes: ['localhost:5000/api/auth', 'localhost:5001/api/auth']
         }
      }),
      BsDropdownModule.forRoot(),
      RouterModule.forRoot(appRoutes),
      FileUploadModule,
      NgbModule,
      NgxInfiniteScrollerModule,
   ],
   providers: [
      AuthService,
      AlertifyService,
      AccountManagementService,
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
      ChatResolver,
      LogsResolver,
      UsersWithRolesResolver,
      AdminService
   ],
   bootstrap: [
      AppComponent
   ],
   entryComponents: [
      RolesModalComponent
   ]
})
export class AppModule { }