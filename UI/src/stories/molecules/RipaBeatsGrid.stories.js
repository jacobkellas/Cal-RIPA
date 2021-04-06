import RipaBeatsGrid from '@/components/molecules/RipaBeatsGrid'
import { beats } from '../data/beats'

export default {
  title: 'Molecules/RipaBeatsGrid',
  component: RipaBeatsGrid,
  parameters: {},
}

export const basic = () => ({
  components: { RipaBeatsGrid },
  data() {
    return {
      data: beats,
    }
  },
  template: '<ripa-beats-grid :items="data"></ripa-beats-grid>',
})