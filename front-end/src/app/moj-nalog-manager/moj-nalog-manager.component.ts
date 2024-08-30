import { Component, OnInit } from '@angular/core';
import {
  GostRezervacijaInfo, Grad, LozinkaRequest, LozinkaResponse,
  PopustResponse, RecenzijaInfoManager, RecenzijaManagerResponse,
  RezervacijaHostesaResponse,
  UposlenikResponse
} from "../KorisnikKlase/KorisnikResponse";
import {HttpClient} from "@angular/common/http";
import {MyAuth} from "../Servisi/MyAuth";
import {MojConfig} from "../Servisi/MojConfig";
import {HostesaManagerEdit, KorisnikEditResponse} from "../KorisnikKlase/ManagerKlase";
import {formatirajBroj, isNullOrEmpty} from "../Helpers/StringHelper";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'app-moj-nalog-manager',
  templateUrl: './moj-nalog-manager.component.html',
  styleUrls: ['./moj-nalog-manager.component.css']
})
export class MojNalogManagerComponent implements OnInit {

  korisnik:UposlenikResponse|undefined;
  username="";
  recenzije: RecenzijaManagerResponse []|undefined;
  recenzija: RecenzijaInfoManager|undefined;
  dialog_detalji=false;
  datumRecenzije = "";
  dialog_lozinka=false;
  lozinka_upozorenje="";
  staraLozinka="";
  novaLozinka ="";
  gradovi:Grad[]=[];
  manager:HostesaManagerEdit|undefined;
  greska="";
  dialog_edit = false;
  promijenjenaSlika=false;
  slikaURL="";
  constructor(private http:HttpClient, private auth:MyAuth, private datePipe:DatePipe) { }
  async preuzmiDetalje (id:number){
    this.recenzija = await this.http.get<RecenzijaInfoManager>(MojConfig.adresa_servera+"GetRecenzijaInfoManager?id="+id,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    this.dialog_detalji =!this.dialog_detalji;
  }
  async preuzmiRecenzije () {
    this.recenzije = await this.http.get<RecenzijaManagerResponse[]>(MojConfig.adresa_servera+"GetRecenzijeManager?datum="+ this.datumRecenzije,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
  }
  async getGradovi() {
    this.gradovi = await this.http.get<Grad[]>(MojConfig.adresa_servera+"GetGradovi").toPromise() as Grad[];
  }
  provjeriPolja() {
    return isNullOrEmpty(this.manager!.ime) || isNullOrEmpty(this.manager!.prezime) ||
      isNullOrEmpty(this.manager!.email) || isNullOrEmpty(this.manager!.brojTelefona) ||
      isNullOrEmpty(this.manager!.strucnaSprema) || this.manager!.gradId<1;
  }
  async sacuvaj() {
    if(this.provjeriPolja()) {
      this.greska="Sva polja su obavezna!";
      return;
    }
    let res:KorisnikEditResponse = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"EditManager",this.manager,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as KorisnikEditResponse;
    if(res.editovan){
      if(this.promijenjenaSlika) {
        let obj = {
          base64String: this.slikaURL,
          korisnikId: this.auth.getToken()!.uposlenik!.korisnikID
        };
        let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"SendProfilna", obj,
          {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
        if(!res!.editovan) {
          alert("Ucitavanje slike nije uspjelo! Greska: " + res?.greska);
        }
        else {
          location.reload();
        }
      }
      await this.ucitajKorisnika();
      this.dialog_edit=!this.dialog_edit;
    }
    else {
      this.greska=res.greska;
    }
  }
  popuniPolja(){
    this.manager = {
      ime:this.korisnik!.ime,
      prezime:this.korisnik!.prezime,
      datumRodjenja:this.korisnik!.datumRodjenja.toString().split("T")[0],
      gradId:this.korisnik!.gradID,
      email:this.korisnik!.email,
      username:this.korisnik!.username,
      brojTelefona: this.korisnik!.brojTelefona,
      datumZaposlenja:this.korisnik!.datumZaposlenja.toString().split("T")[0],
      strucnaSprema:this.korisnik!.strucnaSprema,
      korisnikId:0
    };
  }
  isNullOrEmpty(tekst:string) {
    if(tekst =='') return true;
    let lista = tekst.split(" ");
    let text = lista.join('');
    return text.length==0;
  }
  async promjenaLozinke() {
    if(this.isNullOrEmpty(this.novaLozinka) || this.isNullOrEmpty(this.staraLozinka)) {
      this.lozinka_upozorenje= "Sva polja su obavezna!";
      return;
    }
    if(this.novaLozinka.length<8 || this.staraLozinka.length<8) {
      this.lozinka_upozorenje = "Vasa lozinka mora sadrzavati minimalno 8 karaktera!";
      return;
    }
    let obj: LozinkaRequest = {
      staraLozinka: this.staraLozinka,
      novaLozinka:this.novaLozinka
    };
    let res = await this.http.post<LozinkaResponse>(MojConfig.adresa_servera+"PromjenaLozinke",
      obj,{headers:{"my-token":this.auth.getToken()?.token!}} ).toPromise();
    if(res!.promijenjena) {
      alert("Uspjesno promijenjena lozinka!");
      this.dialog_lozinka = !this.dialog_lozinka;
    }
    else {
      this.staraLozinka="";
      this.novaLozinka="";
      this.lozinka_upozorenje = res!.greska;
    }
  }

  async ucitajKorisnika() {
    this.korisnik = await this.http.get<UposlenikResponse>(MojConfig.adresa_servera+"GetUposlenik",
      {headers:{"my-token":this.auth.getToken()?.token!} }).toPromise();
    this.slikaURL= MojConfig.adresa_servera+"GetProfilnaById?id=" + this.auth.getToken()!.uposlenik!.korisnikID;
  }
  ucitajSliku() {
    let reader = new FileReader();
    // @ts-ignore
    let file = document.getElementById("slika").files[0];
    if(!file) {
      this.slikaURL="";
      this.promijenjenaSlika=false;
    }
    let fileSizeMB = file.size / (1024 * 1024);
    if(fileSizeMB>2) {
      this.slikaURL ="";
      this.promijenjenaSlika =false;
      alert("Velicina slike je prevelika! Maksimalna velicina: 2mb");
      return;
    }
    reader.onload = ()=> {
      this.slikaURL = reader.result!.toString();
      this.promijenjenaSlika=true;
    }

    reader.readAsDataURL(file);
  }
  async ngOnInit() {
    this.podesiVrijeme();
    await this.getGradovi();
    await this.ucitajKorisnika();
    await this.preuzmiRecenzije();
  }
  podesiVrijeme() {
    let currentTime = this.datePipe.transform(new Date(), 'yyyy-MM-dd HH:mm:ss', 'Europe/Sarajevo');
    let datum = new Date(currentTime!);
    this.datumRecenzije = datum.getFullYear() +"-"+formatirajBroj(datum.getMonth()+1)+"-" +
      formatirajBroj(datum.getDate());
  }
  boolstyle (stanje:boolean) {
    if(stanje) return {"color":"green"};
    else return {"color" :"red"};
  }
}
