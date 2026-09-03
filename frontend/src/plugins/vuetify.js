import 'vuetify/styles'
import '@mdi/font/css/materialdesignicons.css'

import { createVuetify } from 'vuetify'
import { aliases, mdi } from 'vuetify/iconsets/mdi'

const forgeflowDark = {
  dark: true,
  colors: {
    background: '#07080B',
    surface: '#141B26',
    'surface-bright': '#232C3A',
    'surface-variant': '#232C3A',
    'on-background': '#F5F5F5',
    'on-surface': '#F5F5F5',
    'on-surface-variant': '#D1D5DB',
    primary: '#22C55E',
    'primary-darken-1': '#16A34A',
    'on-primary': '#111827',
    secondary: '#9CA3AF',
    'on-secondary': '#111827',
    info: '#38BDF8',
    success: '#22C55E',
    warning: '#F59E0B',
    error: '#F87171',
  },
  variables: {
    // Borders are drawn as rgba(border-color, border-opacity); a light hue at low
    // opacity reads as a hairline instead of a hard line.
    'border-color': '#FFFFFF',
    'border-opacity': 0.08,
    'medium-emphasis-opacity': 0.72,
    'disabled-opacity': 0.38,
  },
}

export default createVuetify({
  icons: {
    defaultSet: 'mdi',
    aliases,
    sets: { mdi },
  },
  theme: {
    defaultTheme: 'forgeflowDark',
    themes: { forgeflowDark },
  },
  defaults: {
    VCard: {
      color: 'surface',
      flat: true,
      border: true,
      rounded: 'lg',
    },
    VBtn: {
      rounded: 'lg',
      class: 'text-none',
    },
    VSnackbar: {
      location: 'top',
      timeout: 4000,
    },
    VDataTable: {
      fixedHeader: true,
      fixedFooter: true,
      hover: true,
      density: 'comfortable',
    },
  },
})
