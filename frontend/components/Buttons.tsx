import classNames from 'classnames';
import Link from 'next/link';
import { useCallback, useState } from 'react';
import { useForm, useFormContext } from 'react-hook-form';

const Spinner: React.FC<{ className?: string }> = (props) => (
  <svg
    className={props.className}
    viewBox="0 0 44 44"
    xmlns="http://www.w3.org/2000/svg"
    stroke="#fff"
  >
    <g fill="none" fillRule="evenodd" strokeWidth="2">
      <circle cx="22" cy="22" r="1">
        <animate
          attributeName="r"
          begin="0s"
          dur="1.8s"
          values="1; 20"
          calcMode="spline"
          keyTimes="0; 1"
          keySplines="0.165, 0.84, 0.44, 1"
          repeatCount="indefinite"
        />
        <animate
          attributeName="stroke-opacity"
          begin="0s"
          dur="1.8s"
          values="1; 0"
          calcMode="spline"
          keyTimes="0; 1"
          keySplines="0.3, 0.61, 0.355, 1"
          repeatCount="indefinite"
        />
      </circle>
      <circle cx="22" cy="22" r="1">
        <animate
          attributeName="r"
          begin="-0.9s"
          dur="1.8s"
          values="1; 20"
          calcMode="spline"
          keyTimes="0; 1"
          keySplines="0.165, 0.84, 0.44, 1"
          repeatCount="indefinite"
        />
        <animate
          attributeName="stroke-opacity"
          begin="-0.9s"
          dur="1.8s"
          values="1; 0"
          calcMode="spline"
          keyTimes="0; 1"
          keySplines="0.3, 0.61, 0.355, 1"
          repeatCount="indefinite"
        />
      </circle>
    </g>
  </svg>
);

type ButtonKind = 'primary' | 'secondary';

function buttonClassName(opts: { isLoading?: boolean; kind: ButtonKind }) {
  if (opts.kind === 'secondary') {
    return 'bg-white border border-grey-600 rounded-md shadow-sm py-2 px-4 flex justify-center text-sm font-medium text-gray-600 hover:bg-gray-100 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-gray-500';
  } else if (opts.kind === 'primary') {
    return classNames(
      'border border-transparent rounded-md shadow-sm py-2 px-4 flex justify-center text-sm font-medium text-white  focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500',
      !opts.isLoading && 'bg-indigo-600 hover:bg-indigo-700',
      opts.isLoading && 'bg-gray-600 disabled cursor-wait',
    );
  }
}

export const ActionButton: React.FC<{
  onClick: () => unknown | Promise<unknown>;
  isLoading?: boolean;
  kind: ButtonKind;
}> = (props) => {
  const [isPromiseExecuting, setIsLoading] = useState(false);

  const propsOnClick = props.onClick;
  const onClick = useCallback(async () => {
    setIsLoading(true);
    try {
      await propsOnClick();
    } finally {
      setIsLoading(false);
    }
  }, [propsOnClick, setIsLoading]);

  const isLoading = props.isLoading || isPromiseExecuting;

  return (
    <button
      type="button"
      disabled={isLoading}
      className={buttonClassName({
        isLoading: isLoading,
        kind: props.kind,
      })}
      onClick={onClick}
    >
      {props.children}
    </button>
  );
};

export const LinkButton: React.FC<{ href: string; kind: ButtonKind }> = (
  props,
) => {
  return (
    <Link href={props.href}>
      <a
        className={buttonClassName({
          kind: props.kind,
        })}
      >
        {props.children}
      </a>
    </Link>
  );
};

export const SubmitButton: React.FC<{ kind: ButtonKind }> = (props) => {
  const { formState } = useFormContext();

  return (
    <button
      type="submit"
      disabled={formState.isSubmitting}
      className={buttonClassName({
        isLoading: formState.isSubmitting,
        kind: props.kind,
      })}
    >
      {formState.isSubmitting ? (
        <>
          <Spinner className="absolute animate-spin h-5 w-5" />
          <span className="opacity-0">{props.children}</span>
        </>
      ) : (
        props.children
      )}
    </button>
  );
};
