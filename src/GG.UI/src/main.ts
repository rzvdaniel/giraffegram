import { ApplicationConfig } from '@angular/core';
import { bootstrapApplication } from '@angular/platform-browser';
import { AppComponent } from 'app/app.component';
import { appConfig } from 'app/app.config';
import { RUNTIME_CONFIG } from 'app/runtime.config'

fetch('./assets/config.json')
    .then((resp) => resp.json())
    .then((config) => {
        bootstrapApplication(AppComponent, createApplicationConfig(config))
            .catch(err => console.error(err));
    });

function createApplicationConfig(config: any) : ApplicationConfig {
    const {providers, ...rest} = appConfig;
    return  {
        ...rest,
        providers: [...providers, { provide: RUNTIME_CONFIG, useValue: config }],
    }
}
