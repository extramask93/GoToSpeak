import {Routes} from '@angular/router';
import { HomeComponent } from './home/home.component';
import { AuthGuard } from './_guards/auth.guard';
import { MemberListComponent } from './members/memberList/memberList.component';
import { MemberListResolver } from './_resolvers/member-list.resolver';
import { UsersListComponent } from './users-list/users-list.component';
import { PhotoEditorComponent } from './photo-editor/photo-editor.component';
import { UserResolver } from './_resolvers/user.resolver';

export const appRoutes: Routes = [
    {path: '', component: HomeComponent},
    {path: 'photo', component: PhotoEditorComponent, canActivate: [AuthGuard], resolve: {user: UserResolver}},
    {path: 'users', component: UsersListComponent, canActivate: [AuthGuard]},
    {path: 'members', component: MemberListComponent, canActivate: [AuthGuard]},
    {path: '**', redirectTo: '', pathMatch: 'full'},
];

