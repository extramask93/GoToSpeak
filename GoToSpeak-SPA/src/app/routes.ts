import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/memberList/memberList.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { UsersListComponent } from './users-list/users-list.component';
import { UserResolver } from './_resolvers/user.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver.';
import { WelcomeComponent } from './welcome/welcome.component';
import { LobbyComponent } from './lobby/lobby/lobby.component';
import { ChatResolver } from './_resolvers/chat.resolver';
import { ListResolver } from './_resolvers/list.resolver';
import { RoomResolver } from './_resolvers/room.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { SignInComponent } from './sign-in/sign-in.component';
import { SendResetEmailComponent } from './sign-in/send-reset-email/send-reset-email.component';
import { ResetPasswordComponent } from './sign-in/reset-password/reset-password.component';
import { SignInMfaComponent } from './sign-in/sign-in-mfa/sign-in-mfa.component';
import { MfaSetupComponent } from './settings/mfa-setup/mfa-setup.component';
import { SettingsComponent } from './settings/settings.component';
import { PhotoEditorComponent } from './settings/photo-editor/photo-editor.component';
import { MfaRecoveryCodesComponent } from './settings/mfa-recovery-codes/mfa-recovery-codes.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'lobby', component: LobbyComponent,
    resolve: {messages: ChatResolver, users: ListResolver, rooms: RoomResolver}},
    {path: 'sendEmail', component: SendResetEmailComponent},
    {path: 'resetPassword', component: ResetPasswordComponent},
    {path: 'signin', component: SignInComponent},
    {path: 'signin2fa', component: SignInMfaComponent},
    {path: 'photo', component: PhotoEditorComponent, canActivate: [AuthGuard], resolve: {user: UserResolver}},
    {path: 'users', component: UsersListComponent, canActivate: [AuthGuard]},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard],
     resolve: {users: MemberListResolver, messages: MessagesResolver}},
    {path: 'welcome', component: WelcomeComponent},
    {path: 'admin', component: AdminPanelComponent, canActivate: [AuthGuard], data: {roles: ['Admin', 'Moderator']}},
    {path: 'mfa', component: MfaSetupComponent, canActivate: [AuthGuard]},
    {path: 'mfacodes', component: MfaRecoveryCodesComponent, canActivate: [AuthGuard]},
    {path: 'settings', component: SettingsComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: '', pathMatch: 'full'}
];

