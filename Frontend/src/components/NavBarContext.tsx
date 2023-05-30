import { ParentComponent, createContext, createSignal, useContext } from "solid-js";

export const makeNavBarContext = (initialOpen = false) => {
  const [open, setOpen] = createSignal(initialOpen);
  return [open, setOpen] as const;
  // `as const` forces tuple type inference
};
type NavBarContextType = ReturnType<typeof makeNavBarContext>;
export const NavBarContext = createContext<NavBarContextType>(createSignal(false));
export const useNavBarContext = () => useContext(NavBarContext);

export const OpenToggleProvider: ParentComponent = (props) => {
  const [open, setOpen] = makeNavBarContext();
  return <NavBarContext.Provider value={[open, setOpen]}>{props.children}</NavBarContext.Provider>;
};
