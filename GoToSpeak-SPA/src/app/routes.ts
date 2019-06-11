import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/memberList/memberList.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { UsersListComponent } from './users-list/users-list.component';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'users', component: UsersListComponent, canActivate: [AuthGuard]},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];

