import { Component, OnInit } from '@angular/core';
import {
  GostResponse,
  GostRezervacijaInfo,
  Grad, KorisnikEditRequest, LozinkaRequest, LozinkaResponse,
  Popust, PopustResponse, RecenzijaPost,
  RezervacijaGostResponse, RezervacijaHostesaResponse, UposlenikResponse
} from "../KorisnikKlase/KorisnikResponse";
import {HttpClient} from "@angular/common/http";
import {MyAuth} from "../Servisi/MyAuth";
import {MojConfig} from "../Servisi/MojConfig";
import {Rezultat} from "../AutentifikacijaKlase/LoginRequest";
import {ManuelnaRezPost, RezervacijaResponse, Stol, StoloviRequest, StoloviResponse} from "../RezervacijaKlase/Klase";
import {formatirajBroj, isNullOrEmpty} from "../Helpers/StringHelper";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'app-moj-nalog-hostesa',
  templateUrl: './moj-nalog-hostesa.component.html',
  styleUrls: ['./moj-nalog-hostesa.component.css']
})
export class MojNalogHostesaComponent implements OnInit {


  korisnik:UposlenikResponse|undefined;
  username="";
  popusti:PopustResponse[]|undefined;
  popusti_copy:PopustResponse[] | undefined;
  rezervacije : RezervacijaHostesaResponse []|undefined;
  rezervacija:GostRezervacijaInfo|undefined;
  dialog_detalji=false;
  imeprezime="";
  datumRezervacija = "";
  razlog="";
  dialog_otkaz=false;
  rezervacijaId =0;
  dialog_lozinka=false;
  lozinka_upozorenje="";
  staraLozinka="";
  novaLozinka ="";
  dialog_nova = false;
  nova: ManuelnaRezPost |undefined;
  odLista: number[] = [];
  doLista: number[] = [];
  datumRezervacije = "";
  odSati = 0;
  doSati = 0;
  slobodniStolovi: Stol[] = [];
  greska="";
  constructor(private http:HttpClient, private auth:MyAuth, private datePipe: DatePipe) { }
  async preuzmiDetalje (id:number){
    this.rezervacija = await this.http.get<GostRezervacijaInfo>(MojConfig.adresa_servera+"GetGostRezervacijaInfo?id="
      +id,
      {headers: {"my-token":this.auth.getToken()?.token!}}).toPromise();
    this.dialog_detalji=!this.dialog_detalji;
  }
  pozadina (otkazana:boolean, ponistena:boolean,preuzeta:boolean ) {
    if(otkazana || ponistena) {
      return {
        "background-color":"rgba(255, 0, 0, 0.2)"
      };
    }
    else if(preuzeta) {
      return {
        "background-color":"rgb(221,255,221)"
      };
    }
    else {
      return {
        "background-color":"white"
      };
    }
  }
  async izbrisiRezervaciju () {
    let obj = {
      rezervacijaId: this.rezervacijaId,
      komentar:this.razlog
    };
    let res = await this.http.post
    (MojConfig.adresa_servera+"IzbrisiRezervacijuHostesa",obj,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res) {
      await this.getRezervacije();
      await this.ucitajKorisnika();
      this.dialog_otkaz= !this.dialog_otkaz;
      return;
    }
    else {
      alert("Nije moguce poništiti rezervaciju!");
    }
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
  async getRezervacije() {
    this.rezervacije = await this.http.get<RezervacijaHostesaResponse[]>(MojConfig.adresa_servera+"GetRezervacijeHostesa?datum="
      +this.datumRezervacija,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();

  }
  async ucitajKorisnika() {
    this.korisnik = await this.http.get<UposlenikResponse>(MojConfig.adresa_servera+"GetUposlenik",
      {headers:{"my-token":this.auth.getToken()?.token!} }).toPromise();
  }
  async odobriRezervaciju(id:number) {
    let res = await this.http.get(MojConfig.adresa_servera+"OdobriRezervaciju?id="+id,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res) {
      await this.getRezervacije();
    }
    else {
      alert("U ovom trenutku više nije moguće odobriti rezervaciju!");
      await this.getRezervacije();
    }
  }
  async oznaciPreuzetom (id:number) {
    let res = await this.http.get(MojConfig.adresa_servera+"OznaciPreuzetom?id="+id,
      {headers:{"my-token":this.auth.getToken()?.token!}} ).toPromise();
    if(res) {
      await this.getRezervacije();
    }
    else {
      alert("U ovom trenutku više nije moguće označiti rezervaciju preuzetom!");
    }
  }
  async getPopusti() {
    let putanja ="";
    if(this.isNullOrEmpty(this.imeprezime)) {
      putanja = MojConfig.adresa_servera+"GetPopustiHostesa";
    }
    else {
      putanja = MojConfig.adresa_servera+"GetPopustiHostesa?imePrezime="+this.imeprezime;
    }
    this.popusti_copy = await this.http.get<PopustResponse[]>(putanja,
      {headers:{"my-token": this.auth.getToken()!.token!}}).toPromise();
    this.popusti = this.popusti_copy;
  }
  async iskoristiPopust(id:number) {
    let res = await this.http.get(MojConfig.adresa_servera+"IskoristiPopust?id="+id,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res) {
      await this.getPopusti();
      alert("Uspjesna radnja!");
    }
    else {
      alert("Neuspjesna radnja!");
    }
  }
  async ngOnInit(){
    this.podesiVrijeme();
    this.popuniListu(this.odLista, 9, 22);
    this.popuniListu(this.doLista, 10, 23);
    await this.ucitajKorisnika();
    await this.getPopusti();
    await this.getRezervacije();
  }
  podesiVrijeme () {
    let currentTime = this.datePipe.transform(new Date(), 'yyyy-MM-dd HH:mm:ss', 'Europe/Sarajevo');
    let datum = new Date(currentTime!);
    this.datumRezervacija = datum.getFullYear() +"-"+formatirajBroj(datum.getMonth()+1)+"-" +
      formatirajBroj(datum.getDate());
    this.odSati = ((datum.getHours()+7) >22 || (datum.getHours()+7) <9) ? 9 : datum.getHours()+7;
    this.doSati = ((datum.getHours()+7) >22 || (datum.getHours()+7) <9) ? 10 : datum.getHours()+8;
    if((datum.getHours()+7) >22) {
      datum = new Date(datum.setDate(datum.getDate()+1));
    }
    this.datumRezervacije = datum.getFullYear() +"-"+formatirajBroj(datum.getMonth()+1)+"-" +
      formatirajBroj(datum.getDate());
  }
  boolstyle (stanje:boolean) {
    if(stanje) return {"color":"green"};
    else return {"color" :"red"};
  }

  popuniListu(lista: number[], odSati: number, doSati: number) {
    for (let i = odSati; i <= doSati; i++) {
      lista.push(i);
    }
  }
  async preuzmiStolove() {
    if(this.odSati >= this.doSati) {
      this.greska="Nepravilno postavljena satnica!";
      this.slobodniStolovi= [];
      return;
    }
    else {
      this.greska="";
    }
    let obj: StoloviRequest = {
      datumVrijeme: this.datumRezervacije + "T" + formatirajBroj(this.odSati) + ":00:00.000Z",
      trajanje: this.doSati - this.odSati
    };
    this.slobodniStolovi= (await this.http.post(MojConfig.adresa_servera + "GetSlobodniStolovi", obj,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise() as StoloviResponse).stolovi;
  }
  async dodajRezervaciju() {
    if(isNullOrEmpty(this.nova!.imePrezime) || isNullOrEmpty(this.nova!.brojTelefona)) {
      this.greska="Sva polja su obavezna!";
      return;
    }
    if(this.odSati >= this.doSati) {
      this.greska="Nepravilno postavljena satnica!";
      return;
    }
    if(this.nova!.stolovi.length==0) {
      this.greska="Niste odabrali stolove!";
      return;
    }
    this.nova!.trajanje = this.doSati-this.odSati;
    this.nova!.datumVrijeme = this.datumRezervacije + "T" + formatirajBroj(this.odSati) + ":00:00.000Z";
    let res:RezervacijaResponse = await this.http.post<RezervacijaResponse>(MojConfig.adresa_servera+"KreirajManuelnoRezervaciju",
      this.nova!, {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as RezervacijaResponse;
    if(res.rezervacijaID!=0) {
      this.greska="";
      this.dialog_nova=!this.dialog_nova;
      await this.getRezervacije();
      alert("Uspjesno dodana rezervacija sa ID: "+res.rezervacijaID);
    }
    else {
      this.greska=res.greska;
    }
  }
  async popuniPreuzmi() {
    this.greska=="";
    this.nova = {
      brojTelefona:"",
      imePrezime:"",
      stolovi:[],
      posebneZelje:"",
      datumVrijeme:"",
      trajanje:1
    };
    this.podesiVrijeme();
    await this.preuzmiStolove();
    this.dialog_nova=!this.dialog_nova;
  }

  protected readonly formatirajBroj = formatirajBroj;
}
