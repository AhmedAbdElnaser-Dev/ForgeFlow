import httpClient from './httpClient'

export async function getBuckets() {
  const { data } = await httpClient.get('/buckets')
  return data
}

export async function createBucket({ name, retention }) {
  const { data } = await httpClient.post('/buckets', { name, retention })
  return data
}

export async function deleteBucket(bucketKey) {
  await httpClient.delete(`/buckets/${encodeURIComponent(bucketKey)}`)
}
