import { createContext, useContext } from "solid-js";
import { createStore } from "solid-js/store";

type NavBackContextType = [
  { show: boolean; path: string; text: string },
  { setNavBack: (path: string, text: string) => void; clearNavBack: () => void }
];

const makeNavBackContext = () => {
  const [navBackState, setNavBackState] = createStore({
    show: false,
    path: "",
    text: ""
  });
  const navBack = [
    navBackState,
    {
      setNavBack(path: string, text: string) {
        setNavBackState({ show: true, path: path, text: text });
      },
      clearNavBack() {
        setNavBackState({ show: false, path: "", text: "" });
      }
    }
  ];
  return navBack as NavBackContextType;
};

//const [navBackState, { setNavBack, clearNavBack }] = makeNavBackContext();

//type NavBackContextType = ReturnType<typeof makeNavBackContext>;
export const NavBackContext = createContext(makeNavBackContext());

export function NavBackProvider(props: any) {
  const navBack = makeNavBackContext();
  return <NavBackContext.Provider value={navBack}>{props.children}</NavBackContext.Provider>;
}

export const useNavContext = () => useContext(NavBackContext);
