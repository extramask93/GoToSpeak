import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ContactsComponent } from './contacts/contacts.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/memberList/memberList.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { MessagesResolver } from './_resolvers/messages.resolver.';
import { TempComponent } from './temp/temp.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'temp', component: TempComponent, canActivate: [AuthGuard]},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard], resolve: {users: MemberListResolver}},
    {path: 'contacts', component: ContactsComponent, canActivate: [AuthGuard]},
    {path: 'messages', component: MessagesComponent, canActivate: [AuthGuard], resolve: {messages: MessagesResolver}},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];

