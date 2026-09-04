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

export async function setBucketActivation(bucketKey, isActive) {
  await httpClient.put(`/buckets/${encodeURIComponent(bucketKey)}/activation`, { isActive })
}

export async function deleteBucket(bucketKey) {
  await httpClient.delete(`/buckets/${encodeURIComponent(bucketKey)}`)
}
