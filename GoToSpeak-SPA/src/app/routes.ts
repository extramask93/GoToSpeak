import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ContactsComponent } from './contacts/contacts.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './memberList/memberList.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
    {path: 'contacts', component: ContactsComponent, canActivate: [AuthGuard]},
    {path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];

