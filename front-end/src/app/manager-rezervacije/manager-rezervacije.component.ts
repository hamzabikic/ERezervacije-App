import {Component, HostListener, OnInit} from '@angular/core';
import {HttpClient} from "@angular/common/http";
import {MyAuth} from "../Servisi/MyAuth";
import {GostRezervacijaInfo, RezervacijaHostesaResponse} from "../KorisnikKlase/KorisnikResponse";
import {MojConfig} from "../Servisi/MojConfig";
import {ManuelnaRezPost, RezervacijaResponse, Stol, StoloviRequest, StoloviResponse} from "../RezervacijaKlase/Klase";
import {formatirajBroj, isNullOrEmpty} from "../Helpers/StringHelper";
import {DatePipe} from "@angular/common";

@Component({
  selector: 'app-manager-rezervacije',
  templateUrl: './manager-rezervacije.component.html',
  styleUrls: ['./manager-rezervacije.component.css']
})
export class ManagerRezervacijeComponent implements OnInit {
  rezervacije: RezervacijaHostesaResponse[] = [];
  id="";
  rezervacijaId:number=0;
  razlog="";
  dialog_otkaz=false;
  dialog_detalji=false;
  rezervacija:GostRezervacijaInfo|undefined;
  upozorenje_brisanje=false;
  dialog_nova = false;
  greska="";
  nova: ManuelnaRezPost |undefined;
  datumRezervacije = "";
  odLista: number[] = [];
  doLista: number[] = [];
  odSati: number = 0;
  doSati: number = 0;
  slobodniStolovi: Stol[] = [];
  constructor(private http: HttpClient, private auth: MyAuth, private datePipe:DatePipe ) { }

  async ngOnInit(){
    this.podesiVrijeme();
    this.popuniListu(this.odLista, 9, 22);
    this.popuniListu(this.doLista, 10, 23);
    await this.getRezervacije();
  }
  popuniListu(lista: number[], odSati: number, doSati: number) {
    for (let i = odSati; i <= doSati; i++) {
      lista.push(i);
    }
  }
  podesiVrijeme () {
    let currentTime = this.datePipe.transform(new Date(), 'yyyy-MM-dd HH:mm:ss', 'Europe/Sarajevo');
    let datum = new Date(currentTime!);
    this.odSati = ((datum.getHours()+7) >22 || (datum.getHours()+7) <9) ? 9 : datum.getHours()+7;
    this.doSati = ((datum.getHours()+7) >22 || (datum.getHours()+7) <9) ? 10 : datum.getHours()+8;
    if((datum.getHours()+7) >22) {
      datum = new Date(datum.setDate(datum.getDate()+1));
    }
    this.datumRezervacije = datum.getFullYear() +"-"+formatirajBroj(datum.getMonth()+1)+"-" +
      formatirajBroj(datum.getDate());
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
    this.dialog_nova=!this.dialog_nova
  }
  async getRezervacije () {
    if(this.id==""){
      this.rezervacije = await this.http.get<RezervacijaHostesaResponse[]>(MojConfig.adresa_servera+"GetRezervacijeManager",
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as RezervacijaHostesaResponse[]; }
    else {
      this.rezervacije = await this.http.get<RezervacijaHostesaResponse[]>(MojConfig.adresa_servera+"GetRezervacijeManager?id="
        + this.id,
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as RezervacijaHostesaResponse[];
    }

  }
  async preuzmiDetalje (id:number){
    this.rezervacija = await this.http.get<GostRezervacijaInfo>(MojConfig.adresa_servera+"GetGostRezervacijaInfo?id="
      +id,
      {headers: {"my-token":this.auth.getToken()?.token!}}).toPromise();
    this.dialog_detalji=!this.dialog_detalji;
  }
  async izbrisiRezervaciju () {
    let obj = {
      rezervacijaId: this.rezervacijaId,
      komentar: this.razlog
    };
    let res = await this.http.post
    (MojConfig.adresa_servera + "IzbrisiRezervacijuHostesa", obj,
      {headers: {"my-token": this.auth.getToken()?.token!}}).toPromise();
    if (res) {
      await this.getRezervacije();
      this.dialog_otkaz = !this.dialog_otkaz;
      return;
    } else {
      alert("Nije moguce poništiti rezervaciju!");
    }
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
    async obrisiRezervaciju(id:number){
       let res = await this.http.get(MojConfig.adresa_servera+"ObrisiRezervacijuManager?id="
       +id, {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
       if(res) {
         await this.getRezervacije();
         this.upozorenje_brisanje=!this.upozorenje_brisanje;
       }
       else {
         alert("Neuspjesno brisanje rezervacije!");
       }
    }
  boolstyle (stanje:boolean) {
    if(stanje) return {"color":"green"};
    else return {"color" :"red"};
  }

  provjera() {

    this.vidljiva_tabela = window.innerWidth > 1300;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event:Event) {
    this.provjera();
  }

  vidljiva_tabela = window.innerWidth > 1300;
  protected readonly formatirajBroj = formatirajBroj;
}

