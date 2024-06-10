import { bootstrapApplication } from '@angular/platform-browser';
import { appConfig } from './app/app.config';
import { AppComponent } from './app/app.component';
import { API_URL, AppSettings } from './configuration/configuration';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';

async function getSettings(): Promise<AppSettings> {
  const res = await fetch(`assets/appsettings.json?t=${new Date().getTime()}`);
  return await res.json();
}

async function bootstrapWrapper() {
  const appSettings = await getSettings();

  const _appConfig = {...appConfig, providers: [...appConfig.providers, 
    {
      provide: AppSettings,
      useValue: appSettings
    },
    {
      provide: API_URL,
      useValue: appSettings.apiUrl
    }
  ]}

  bootstrapApplication(AppComponent, _appConfig)
  .catch((err) => console.error(err));
}

bootstrapWrapper();