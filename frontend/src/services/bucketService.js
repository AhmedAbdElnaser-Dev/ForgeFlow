import httpClient from './httpClient'

export async function getBuckets() {
  const { data } = await httpClient.get('/buckets')
  return data
}

// Active buckets only, readable by any signed-in user.
export async function getFolders() {
  const { data } = await httpClient.get('/folders')
  return data
}

export async function createBucket({ name, retention }) {
  const { data } = await httpClient.post('/buckets', { name, retention })
  return data
}

export async function setBucketActivation(name, isActive) {
  await httpClient.put(`/buckets/${encodeURIComponent(name)}/activation`, { isActive })
}

export async function deleteBucket(name) {
  await httpClient.delete(`/buckets/${encodeURIComponent(name)}`)
}
