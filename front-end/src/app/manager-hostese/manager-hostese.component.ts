import {Component, HostListener, OnInit} from '@angular/core';
import {
  AddHostesaResponse,
  GostManagerResponse, HostesaAddManager,
  HostesaManagerEdit,
  HostesaManagerResponse, KorisnikEditResponse,
  KorisnikManagerEdit
} from "../KorisnikKlase/ManagerKlase";
import {Grad} from "../KorisnikKlase/KorisnikResponse";
import {MyAuth} from "../Servisi/MyAuth";
import {HttpClient} from "@angular/common/http";
import {MojConfig} from "../Servisi/MojConfig";
import {isNullOrEmpty} from "../Helpers/StringHelper";

@Component({
  selector: 'app-manager-hostese',
  templateUrl: './manager-hostese.component.html',
  styleUrls: ['./manager-hostese.component.css']
})
export class ManagerHosteseComponent implements OnInit {

  imePrezime="";
  hostese:HostesaManagerResponse[] =[];
  dialog_edit =false;
  editovanje = false;
  naslov ="detalji";
  trenutni_gost:HostesaManagerResponse|undefined;
  ime="";
  prezime="";
  datumRodjenja:string="";
  brojTelefona="";
  email="";
  username="";
  gradId=0;
  gradovi:Grad[]=[];
  datumZaposlenja="";
  strucnaSprema ="";
  korisnikId =0;
  greska="";
  upozorenje_brisanje =false;
  novaLozinka_dialog = false;
  dialog_nova=false;
  hostesa:HostesaAddManager|undefined;
  slikaURL="";
  promijenjena=false;
  novaSlikaURL="";
  constructor(private auth: MyAuth, private http:HttpClient) { }

  async ngOnInit(){
    await this.getGradovi();
    await this.getHostese();
  }

  ucitajSliku() {
    let reader = new FileReader();
    // @ts-ignore
    let file = document.getElementById("slika").files[0];
    if(!file) {
      this.slikaURL="";
      this.promijenjena=false;
      return;
    }
    let fileSizeMB = file.size / (1024 * 1024);
    if(fileSizeMB>2) {
      this.slikaURL ="";
      this.promijenjena =false;
      alert("Velicina slike je prevelika! Maksimalna velicina: 2mb");
      return;
    }
    reader.onload = ()=> {
      this.slikaURL = reader.result!.toString();
      this.promijenjena=true;
    }

    reader.readAsDataURL(file);
  }

  ucitajNovuSliku() {
    let reader = new FileReader();
    // @ts-ignore
    let file = document.getElementById("novaSlika").files[0];
    if(!file) {
      this.novaSlikaURL="";
      return;
    }
    let fileSizeMB = file.size / (1024 * 1024);
    if(fileSizeMB>2) {
      this.novaSlikaURL ="";
      alert("Velicina slike je prevelika! Maksimalna velicina: 2mb");
      return;
    }
    reader.onload = ()=> {
      this.novaSlikaURL = reader.result!.toString();
    }

    reader.readAsDataURL(file);
  }
  async resetujLozinku(id:number) {
    let res = await this.http.get(MojConfig.adresa_servera+"GenerisiNovuLozinku?korisnikId="
      +id,{headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if (res) {
      this.novaLozinka_dialog=!this.novaLozinka_dialog;
    }
    else {
      alert("Neuspjesno resetovanje lozinke!");
    }
  }
  provjeraPolja() {
    return isNullOrEmpty(this.ime) || isNullOrEmpty(this.prezime) || isNullOrEmpty(this.username) ||
      isNullOrEmpty(this.email) || isNullOrEmpty(this.brojTelefona) || isNullOrEmpty(this.strucnaSprema) ||this.gradId==0;
  }
  provjeriZaNovu() {
    return isNullOrEmpty(this.hostesa!.ime) || isNullOrEmpty(this.hostesa!.prezime) || isNullOrEmpty(this.hostesa!.username) ||
      isNullOrEmpty(this.hostesa!.email) || isNullOrEmpty(this.hostesa!.brojTelefona) ||
      isNullOrEmpty(this.hostesa!.strucnaSprema) ||this.hostesa!.gradID==0;
  }
  async popuniHostesu() {
    this.hostesa = {
      ime:"",
      prezime:"",
      datumRodjenja:new Date(),
      strucnaSprema:"",
      datumZaposlenja:new Date(),
      gradID:0,
      brojTelefona:"",
      email:"",
      username:""
    };
    this.novaSlikaURL="";
    this.greska="";
    this.dialog_nova=!this.dialog_nova;
  }
  async dodajHostesu() {
    if(this.provjeriZaNovu()) {
      this.greska="Sva polja su obavezna!";
      return;
    }
    let res:AddHostesaResponse = await this.http.post<AddHostesaResponse>(MojConfig.adresa_servera+"AddHostesaManager",
      this.hostesa,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as AddHostesaResponse;
    if(res.hostesaId!=0) {
      await this.slanjeNoveSlike(res.hostesaId);
      await this.getHostese();
      this.dialog_nova = !this.dialog_nova;
    }
    else {
      this.greska=res.greska!;
    }
  }
  async slanjeSlike() {
    if(this.promijenjena) {
      let obj = {
        base64String: this.slikaURL,
        korisnikId: this.korisnikId
      };
      let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"SendProfilna", obj,
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
      if(!res?.editovan) {
        alert("Ucitavanje slike nije uspjelo! Greska: " + res?.greska);
      }
      else {
        location.reload();
      }
    }
  }
  async slanjeNoveSlike(id:number) {
    if(this.novaSlikaURL!="") {
      let obj = {
        base64String: this.novaSlikaURL,
        korisnikId: id
      };
      let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"SendProfilna", obj,
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
      if(!res?.editovan) {
        alert("Ucitavanje slike nije uspjelo! Greska: " + res?.greska);
      }
    }
  }
  async sacuvaj () {
    if(this.provjeraPolja()) {
      this.greska="Sva polja su obavezna!";
      return;
    }
    let obj :HostesaManagerEdit = {
      ime:this.ime,
      prezime:this.prezime,
      datumRodjenja:this.datumRodjenja,
      brojTelefona:this.brojTelefona,
      korisnikId:this.korisnikId,
      email:this.email,
      gradId:this.gradId,
      username:this.username,
      strucnaSprema:this.strucnaSprema,
      datumZaposlenja:this.datumZaposlenja
    };
    let res = await this.http.post<KorisnikEditResponse>(MojConfig.adresa_servera+"EditHostesaManager",obj,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res?.editovan!){
      await this.slanjeSlike();
      this.dialog_edit=!this.dialog_edit;
      await this.getHostese();
    }
    else {
      this.greska=res?.greska!;
    }
  }
  popuni(gost:HostesaManagerResponse) {
    this.ime = gost.ime;
    this.prezime = gost.prezime;
    this.email= gost.email;
    this.brojTelefona = gost.brojTelefona;
    this.username = gost.username;
    this.gradId = gost.gradID;
    this.datumRodjenja = gost.datumRodjenja.toString().split("T")[0];
    this.datumZaposlenja = gost.datumZaposlenja.toString().split("T")[0];
    this.strucnaSprema= gost.strucnaSprema;
    this.korisnikId= gost.korisnikID;
    this.slikaURL=MojConfig.adresa_servera+"GetProfilnaById?id="+ gost.korisnikID;
    this.promijenjena=false;
  }
  async obrisiHostesu(id:number) {
    let res= await this.http.get(MojConfig.adresa_servera+"IzbrisiKorisnikManager?id="+id,
      {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise();
    if(res) {
      this.upozorenje_brisanje=!this.upozorenje_brisanje;
      await this.getHostese();
    }
    else {
      alert("Neuspjesno brisanje gosta!");
    }
  }
  async getGradovi() {
    this.gradovi= await this.http.get<Grad[]>(MojConfig.adresa_servera+"GetGradovi").toPromise() as Grad[];
  }
  async getHostese() {
    if (this.imePrezime=="") {
      this.hostese = await this.http.get<HostesaManagerResponse[]>(MojConfig.adresa_servera+"GetHosteseManager",
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as HostesaManagerResponse[];
      return;
    }
    else {
      this.hostese = await this.http.get<HostesaManagerResponse[]>(MojConfig.adresa_servera+"GetHosteseManager?imePrezime="+
        this.imePrezime,
        {headers:{"my-token":this.auth.getToken()?.token!}}).toPromise() as HostesaManagerResponse[];
    }
  }
  provjera() {
    this.strucna_omogucena= window.innerWidth>1500;
    this.datum_omogucen = window.innerWidth>1280;
    this.slika_omogucena = window.innerWidth > 1120;
    this.vidljiva_tabela = window.innerWidth > 1020;
  }

  @HostListener('window:resize', ['$event'])
  onResize(event:Event) {
    this.provjera();
  }

  strucna_omogucena= window.innerWidth>1500;
  datum_omogucen = window.innerWidth>1280;
  slika_omogucena = window.innerWidth > 1120;
  vidljiva_tabela = window.innerWidth > 1020;

  protected readonly MojConfig = MojConfig;
}
