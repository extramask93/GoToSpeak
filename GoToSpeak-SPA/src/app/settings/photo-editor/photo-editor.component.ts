import { Component, OnInit } from '@angular/core';

import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { ActivatedRoute } from '@angular/router';
import { ChatService } from 'src/app/_services/chat.service';
import { AuthService } from 'src/app/_services/auth.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {
  public uploader: FileUploader;
  public hasBaseDropZoneOver = false;
  baseUrl = environment.apiUrl;

  constructor(private chatService: ChatService, public authService: AuthService,
              private alertify: AlertifyService, private route: ActivatedRoute) { }

  ngOnInit() {

    this.initializeUploader();
  }
  public fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({url: this.baseUrl + 'users/' + this.authService.decodedToken.nameid + '/photo',
    authToken: 'Bearer ' + localStorage.getItem('token'),
    isHTML5: true,
    allowedFileType: ['image'],
    removeAfterUpload: true,
    autoUpload: false,
    maxFileSize: 10 * 1024 * 1024
  });
    this.uploader.onAfterAddingFile = (file) => {file.withCredentials = false; };
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const res: Photo = JSON.parse(response);
        this.authService.currentUser.photoUrl = res.photoUrl;
        localStorage.setItem('user', JSON.stringify(this.authService.currentUser));
        this.authService.changeMemberPhoto(res.photoUrl);
      }
    };
  }
}
