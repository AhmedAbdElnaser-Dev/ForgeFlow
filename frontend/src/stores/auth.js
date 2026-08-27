import { ref } from 'vue'
import { defineStore } from 'pinia'

const SESSION_KEY = 'forgeflow.authenticated'

// Placeholder session state until the Autodesk OAuth flow replaces it.
export const useAuthStore = defineStore('auth', () => {
  const isAuthenticated = ref(sessionStorage.getItem(SESSION_KEY) === 'true')

  function signIn() {
    isAuthenticated.value = true
    sessionStorage.setItem(SESSION_KEY, 'true')
  }

  function signOut() {
    isAuthenticated.value = false
    sessionStorage.removeItem(SESSION_KEY)
  }

  return { isAuthenticated, signIn, signOut }
})
