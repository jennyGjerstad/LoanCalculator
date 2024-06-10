import { InjectionToken } from "@angular/core";

export class AppSettings {
    apiUrl: string = "";
}

export const API_URL = new InjectionToken<string>('apiUrl');