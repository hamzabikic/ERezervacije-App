import {Injectable} from "@angular/core";
import {TokenInformacije} from "../AutentifikacijaKlase/LoginRequest";

@Injectable()
export class MyAuth {
  token:string|null = null
  preuzmiToken() {
   this.token= localStorage.getItem("my-token");
  }
  getId():number {
    if(this.isPrijavljen()) {
      if(this.getToken()?.isGost) {
        return this.getToken()?.gost?.korisnikID!;
      }
      else {
        return this.getToken()?.uposlenik?.korisnikID!;
      }
    }
    return 0;
  }
  getIme():string {
    if(this.isPrijavljen()) {
      if(this.getToken()?.isGost) {
        return this.getToken()?.gost?.ime!;
      }
      else {
        return this.getToken()?.uposlenik?.ime!;
      }
    }
    return "";
  }
  getToken() :TokenInformacije | null {
    if(this.isPrijavljen()){
    return JSON.parse(this.token!) as TokenInformacije }
    return null;
  }
    isPrijavljen() {
        this.preuzmiToken();
        if(this.token == null) return false;
        return true;
    }
    isGost() {
        if(this.isPrijavljen()){
          let authobj = JSON.parse(this.token!) as TokenInformacije;
          if(authobj.isGost) return true;
        }
        return false;
    }
    isHostesa() {
      if(this.isPrijavljen()){
        let authobj = JSON.parse(this.token!) as TokenInformacije;
        if(authobj.isHostesa) return true;
      }
      return false;
    }
    isManager() {
      if(this.isPrijavljen()){
        let authobj = JSON.parse(this.token!) as TokenInformacije;
        if(authobj.isManager) return true;
      }
      return false;
    }
}
