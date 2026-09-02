import { computed, ref } from 'vue'
import { defineStore } from 'pinia'
import * as authService from '@/services/authService'

export const useAuthStore = defineStore('auth', () => {
  const user = ref(null)
  const isSessionRestored = ref(false)

  const isAuthenticated = computed(() => user.value !== null)

  function hasAnyRole(roles) {
    return roles.some((role) => user.value?.roles?.includes(role))
  }

  /** Asks the API who we are. Runs once per page load, before the first guarded route. */
  async function restoreSession() {
    if (isSessionRestored.value) {
      return
    }

    try {
      user.value = await authService.getCurrentUser()
    } finally {
      isSessionRestored.value = true
    }
  }

  async function signIn(credentials) {
    user.value = await authService.login(credentials)
    isSessionRestored.value = true
  }

  async function signOut() {
    await authService.logout()
    user.value = null
  }

  return { user, isAuthenticated, isSessionRestored, hasAnyRole, restoreSession, signIn, signOut }
})
