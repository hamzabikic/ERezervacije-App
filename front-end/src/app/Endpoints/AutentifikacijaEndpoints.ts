import {Injectable} from "@angular/core";
import {
  Grad,
  OdjavaResponse,
  PrijavaRequest,
  RegistracijaResponse,
  Rezultat,
  TokenInformacije
} from "../AutentifikacijaKlase/LoginRequest";
import {HttpClient} from "@angular/common/http";
import {MojConfig} from "../Servisi/MojConfig";
import {Observable} from "rxjs";
import {MyAuth} from "../Servisi/MyAuth";


@Injectable()
export class AutentifikacijaEndpoints{
  constructor(private http: HttpClient,private auth:MyAuth) {
  }
   Login (prijava:PrijavaRequest):Observable<TokenInformacije> {
    return this.http.post<TokenInformacije>(MojConfig.adresa_servera+"Login",prijava);
   }
   Logout () :Observable<OdjavaResponse>{
    return this.http.get<OdjavaResponse>(MojConfig.adresa_servera+"Odjava",
      {headers:{
        "my-token": this.auth.getToken()!.token!
        }});
   }
   GetGradovi ():Observable<Grad[]> {
     return this.http.get<Grad[]>(MojConfig.adresa_servera+"GetGradovi");
   }
   PostojiEmail (text:string):Observable<Rezultat> {
    return this.http.get<Rezultat>(MojConfig.adresa_servera+"PostojiEmail?email="+text);
   }
  PostojiUsername (text:string):Observable<Rezultat> {
    return this.http.get<Rezultat>(MojConfig.adresa_servera+"PostojiUsername?username="+text);
  }
  PostojiBrojTelefona(text:string):Observable<Rezultat> {
    return this.http.get<Rezultat>(MojConfig.adresa_servera+"PostojiBrojTelefona?telefon="+text);
  }
  Registracija(obj:PrijavaRequest):Observable<RegistracijaResponse>{
    return this.http.post<RegistracijaResponse>(MojConfig.adresa_servera+"Registracija", obj);
  }

}
