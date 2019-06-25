import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/memberList/memberList.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { UsersListComponent } from './users-list/users-list.component';
import { PhotoEditorComponent } from './photo-editor/photo-editor.component';
import { UserResolver } from './_resolvers/user.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver.';
import { WelcomeComponent } from './welcome/welcome.component';
import { LobbyComponent } from './lobby/lobby/lobby.component';
import { ChatResolver } from './_resolvers/chat.resolver';
import { ListResolver } from './_resolvers/list.resolver';
import { RoomResolver } from './_resolvers/room.resolver';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { SignInComponent } from './sign-in/sign-in.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'lobby', component: LobbyComponent,
    resolve: {messages: ChatResolver, users: ListResolver, rooms: RoomResolver}},
    {path: 'signin', component: SignInComponent},
    {path: 'photo', component: PhotoEditorComponent, canActivate: [AuthGuard], resolve: {user: UserResolver}},
    {path: 'users', component: UsersListComponent, canActivate: [AuthGuard]},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard],
     resolve: {users: MemberListResolver, messages: MessagesResolver}},
    {path: 'welcome', component: WelcomeComponent},
    {path: 'admin', component: AdminPanelComponent, canActivate: [AuthGuard], data: {roles: ['Admin', 'Moderator']}},
    {path: '**', redirectTo: '', pathMatch: 'full'}
];

