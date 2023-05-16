import { createResource, type Component } from 'solid-js';
import { NavBar } from './components/NavBar';

const fetchData = async () => (await fetch("https://localhost:7160/api/v1/routes/klax/ksfo")).json();


const App: Component = () => {
  
  const [data] = createResource(fetchData);
  
  
  return (
    <span>
      {JSON.stringify(data(), null, 2)}
    </span>
  );
};

export default App;
