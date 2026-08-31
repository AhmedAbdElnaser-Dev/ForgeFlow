import httpClient from './httpClient'

export async function login(credentials) {
  const { data } = await httpClient.post('/auth/login', credentials)
  return data
}

export async function logout() {
  await httpClient.post('/auth/logout')
}

/** Returns the signed-in user, or null when there is no valid session. */
export async function getCurrentUser() {
  try {
    const { data } = await httpClient.get('/auth/me')
    return data
  } catch (error) {
    if (error.response?.status === 401) {
      return null
    }
    throw error
  }
}
