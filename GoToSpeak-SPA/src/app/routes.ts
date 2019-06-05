import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { ContactsComponent } from './contacts/contacts.component';
import { MessagesComponent } from './messages/messages.component';
import { AuthGuard } from './_guards/auth.guard';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'contacts', component: ContactsComponent, canActivate: [AuthGuard]},
    {path: 'messages', component: MessagesComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];

