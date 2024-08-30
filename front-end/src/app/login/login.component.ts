import { Component, OnInit } from '@angular/core';
import {Router} from "@angular/router";
import {AutentifikacijaEndpoints} from "../Endpoints/AutentifikacijaEndpoints";
import {PrijavaRequest, TokenInformacije} from "../AutentifikacijaKlase/LoginRequest";
import {MyAuth} from "../Servisi/MyAuth";
import {isNullOrEmpty} from "../Helpers/StringHelper";

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent implements OnInit {

  constructor(private router:Router, private endpoints:AutentifikacijaEndpoints,private auth:MyAuth) { }
  username="";
  password ="";
  greska ="";
  ngOnInit(): void {
  }
   otvoriReg() {
    this.router.navigate(["/registracija"]);
   }

  async prijava() {
    if(isNullOrEmpty(this.username) || isNullOrEmpty(this.password)) {
        this.greska = "Obavezna polja!";
        return;
    }
    if(this.password.length<8) {
      this.greska="Lozinka sadrzava minimalno 8 karaktera!";
      this.password="";
      return;
    }
    let prijava:PrijavaRequest = {
      username:this.username,
      password:this.password
    };
    let token = await this.endpoints.Login(prijava).toPromise();
      if(token!.isLogiran){
      localStorage.setItem("my-token",JSON.stringify(token!));
      this.greska="";
      if(this.auth.isHostesa()){
        this.router.navigate(["/moj-nalog-hostesa"]);
      }
      else if(this.auth.isGost()){
      this.router.navigate(["/moj-nalog-gost"]); }
      else {
        this.router.navigate(["/moj-nalog-manager"]);
      }
      }
      else {
        this.greska = "Neispravan username ili password!"
        this.password ="";
      }
  }


}
