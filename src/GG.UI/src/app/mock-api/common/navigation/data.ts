/* eslint-disable */
import { FuseNavigationItem } from '@fuse/components/navigation';

export const defaultNavigation: FuseNavigationItem[] = [
    {
        id   : 'home',
        title: 'Home',
        type : 'basic',
        icon : 'heroicons_outline:home',
        link : '/dashboards/analytics'
    },
    {
        id   : 'emails',
        title: 'Email Templates',
        type : 'basic',
        icon : 'feather:mail',
        link : '/emails'
    },
    {
        id   : 'apikeys',
        title: 'Api Keys',
        type : 'basic',
        icon : 'feather:key',
        link : '/apikeys'
    }
];
export const adminNavigation: FuseNavigationItem[] = [
    {
        id   : 'users',
        title: 'Users',
        type : 'basic',
        icon : 'feather:mail',
        link : '/users'
    },
];
